import { cookies } from "next/headers";

const API_BASE_URL = process.env.API_BASE_URL || "https://localhost:7287";

export async function sendRequest(
  method: string,
  url: string,
  body?: any,
  isAuth = true,
  contentType: string | null = "application/json"
): Promise<Response> {
  const headers: HeadersInit = {};
  if (contentType) {
    headers["Content-Type"] = contentType;
  }

  if (isAuth) {
    const cookieStore = await cookies();
    const token = cookieStore.get("access_token")?.value;
    if (token) {
      headers["Authorization"] = `Bearer ${token}`;
    }
  }

  const options: RequestInit = {
    method,
    headers,
  };

  if (body !== undefined) {
    options.body = contentType === "application/json" ? JSON.stringify(body) : body;
  }

  return fetch(`${API_BASE_URL}/${url}`, options);
}

export async function getAsync<TResponse>(url: string, isAuth = true): Promise<TResponse | null> {
  try {
    const response = await sendRequest("GET", url, undefined, isAuth);
    return response.json() as Promise<TResponse>;
  } catch (error) {
    console.error(`GET request error for ${url}:`, error);
    return null;
  }
}

export async function postAsync<TRequest, TResponse>(
  url: string,
  data: TRequest,
  isAuth = true
): Promise<TResponse | null> {
  try {
    const response = await sendRequest("POST", url, data, isAuth);
    return response.json() as Promise<TResponse>;
  } catch (error) {
    console.error(`POST request error for ${url}:`, error);
    return null;
  }
}

export async function postMultipartAsync<TResponse>(
  url: string,
  formData: FormData,
  isAuth = true
): Promise<TResponse | null> {
  try {
    const response = await sendRequest("POST", url, formData, isAuth, null);
    return response.json() as Promise<TResponse>;
  } catch (error) {
    console.error(`POST multipart request error for ${url}:`, error);
    return null;
  }
}
