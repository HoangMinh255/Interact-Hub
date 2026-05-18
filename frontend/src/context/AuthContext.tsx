import { createContext, useContext, useState, useEffect } from "react";
import type { ReactNode } from "react";
import type { AuthState, User } from "../types";
import { usersAPI } from "../api";


interface AuthContextType extends AuthState {
  login: (token: string, user: User) => void;
  refreshUser: () => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [auth, setAuth] = useState<AuthState>({
    user: null,
    token: localStorage.getItem("token"),
    isAuthenticated: !!localStorage.getItem("token"),
  });

  const refreshUser = async () => {
    const token = localStorage.getItem("token");
    if (!token) return;

    try {
      const response = await usersAPI.getUser();
      const u = response.data?.data;
      if (!u) return;

      setAuth((prev) => ({
        ...prev,
        isAuthenticated: true,
        user: {
          id: u.id,
          fullName: u.fullName,
          userName: u.userName,
          email: u.email,
          avatarUrl: u.avatarUrl,
          followersCount: u.followersCount ?? 0,
          bio: u.bio,
        },
      }));
    } catch {
      localStorage.removeItem("token");
      setAuth({ user: null, token: null, isAuthenticated: false });
    }
  };

  useEffect(() => {
    void refreshUser();
  }, []);

  const login = (token: string, user: User) => {
    localStorage.setItem("token", token);
    setAuth({ user, token, isAuthenticated: true });
  };

  const logout = () => {
    localStorage.removeItem("token");
    setAuth({ user: null, token: null, isAuthenticated: false });
  };

  return (
    <AuthContext.Provider value={{ ...auth, login, refreshUser, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
};