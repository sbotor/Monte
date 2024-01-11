export type UserRole = 'monte_admin' | 'monte_user';

export const userRoles = {
  admin: 'monte_admin' as UserRole,
  user: 'monte_user' as UserRole
} as const;
