export interface UserDto {
  userId: number;
  username: string;
  email: string;
  fullName: string;
}

// DTO dla danych przekazywanych do serwisów, jeśli to konieczne (np. przy tworzeniu nowego użytkownika).
export interface UserCreateDto {
    username: string;
    email: string;
    fullName: string;
}