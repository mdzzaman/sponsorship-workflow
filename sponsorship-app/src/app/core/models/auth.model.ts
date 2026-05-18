export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: string;
  email: string;
  fullName: string;
  role: string;
  expiresAt: string;
}

export interface CurrentUser {
  userId: string;
  email: string;
  fullName: string;
  role: string;
  token: string;
}
