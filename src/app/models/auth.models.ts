// Types used by authentication service

export interface RegisterDto {
  fullName: string;
  email: string;
  password: string;
  role: 'Renter' | 'Owner';
  phone?: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponse {
  id: string;
  token: string;
  fullName: string;
  role: 'Renter' | 'Owner';
  email: string;
}
