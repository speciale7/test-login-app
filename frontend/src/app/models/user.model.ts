import { UserRole } from './auth.model';

export interface UserDto {
  id: number;
  username: string;
  email: string;
  role: UserRole;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUserDto {
  username: string;
  email: string;
  password: string;
  role: UserRole;
}

export interface UpdateUserDto {
  username?: string;
  email?: string;
  password?: string;
  role?: UserRole;
}
