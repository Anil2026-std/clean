"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { postAsync, getAsync } from "@/services/apiHandler";

export interface ActionResponse {
  success: boolean;
  message?: string;
  errors?: Record<string, string[]>;
}

export async function loginAction(
  prevState: ActionResponse | null,
  formData: FormData
): Promise<ActionResponse> {
  const username = formData.get("username") as string;
  const password = formData.get("password") as string;

  if (!username || !password) {
    return {
      success: false,
      message: "Username and password are required.",
    };
  }

  const response = await postAsync<any, any>("api/Auth/login", { username, password }, false);

  if (response && response.isSuccess && response.data) {
    const { accessToken } = response.data;
    const cookieStore = await cookies();
    
    // Determine expiration from token
    let maxAge = 60 * 60 * 24; // default 24 hours
    try {
      const parts = accessToken.split(".");
      if (parts.length === 3) {
        const payload = JSON.parse(Buffer.from(parts[1], "base64").toString("utf-8"));
        if (payload.exp) {
          const now = Math.floor(Date.now() / 1000);
          maxAge = payload.exp - now;
        }
      }
    } catch (e) {
      console.error("Error decoding JWT in login action:", e);
    }

    cookieStore.set("access_token", accessToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "strict",
      maxAge: maxAge,
      path: "/",
    });
  } else {
    return {
      success: false,
      message: response?.message || "Invalid username or password.",
    };
  }

  // Perform redirect outside of try/catch to avoid Next.js redirect errors
  redirect("/list");
}

export async function logoutAction() {
  const cookieStore = await cookies();
  cookieStore.delete("access_token");
  redirect("/login");
}

export async function createUserAction(
  prevState: ActionResponse | null,
  formData: FormData
): Promise<ActionResponse> {
  const username = formData.get("username") as string;
  const password = formData.get("password") as string;
  const file = formData.get("file") as File | null;

  if (!username || !password) {
    return {
      success: false,
      message: "Username and password are required.",
    };
  }

  let uploadedImageUrl: string | null = null;

  // 1. Handle file upload if present
  if (file && file.size > 0) {
    try {
      // Get upload token from backend
      const tokenRes = await getAsync<any>("api/Media/get-token", true);
      if (!tokenRes || !tokenRes.accessToken) {
        return {
          success: false,
          message: "Failed to obtain upload token from Media Server.",
        };
      }

      const imageServerUrl = process.env.IMAGE_SERVER_URL || "https://localhost:7180";
      const uploadFormData = new FormData();
      uploadFormData.append("file", file, file.name);

      const response = await fetch(`${imageServerUrl}/api/Media/upload?folderName=avatars`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${tokenRes.accessToken}`,
        },
        body: uploadFormData,
      });

      if (response.ok) {
        const uploadResult = await response.json();
        uploadedImageUrl = uploadResult?.mediaUrl || null;
      } else {
        const errText = await response.text();
        return {
          success: false,
          message: `Media Server Upload Failed: ${errText}`,
        };
      }
    } catch (error: any) {
      return {
        success: false,
        message: `Error uploading avatar: ${error.message || error}`,
      };
    }
  }

  // 2. Register user in API backend
  const responseUser = await postAsync<any, any>(
    "api/User",
    {
      username,
      password,
      imageUrl: uploadedImageUrl,
    },
    true
  );

  if (responseUser && responseUser.isSuccess) {
    const { revalidatePath } = await import("next/cache");
    revalidatePath("/list");
    return {
      success: true,
      message: `User '${responseUser.data?.username}' created successfully!`,
    };
  } else {
    // Check for validation errors structure
    const errors = responseUser?.errors;
    return {
      success: false,
      message: responseUser?.message || "Failed to register user.",
      errors: errors,
    };
  }
}
