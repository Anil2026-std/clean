interface ApiResponse<T> {
  isSuccess: boolean;
  message?: string;
  data?: T;
}

interface UserDto {
  id: string;
  username: string;
  imageUrl?: string | null;
}