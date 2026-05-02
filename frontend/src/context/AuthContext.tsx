import { createContext, useContext, useState, useEffect } from "react";
import type { ReactNode } from "react";
import type { AuthState, User } from "../types";
import { usersAPI } from "../api";


interface AuthContextType extends AuthState {
  login: (token: string, user: User) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [auth, setAuth] = useState<AuthState>({
    user: null,
    token: localStorage.getItem("token"),
    isAuthenticated: !!localStorage.getItem("token"),
  });

  useEffect(() => {
  const token = localStorage.getItem("token");
  if (!token) return;

  usersAPI.getUser()
    .then((response) => {
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
          followersCount: u.followersCount ?? 0,
        },
      }));
    })
    .catch(() => {
      // Token is invalid or expired — log out
      localStorage.removeItem("token");
      setAuth({ user: null, token: null, isAuthenticated: false });
    });
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
    <AuthContext.Provider value={{ ...auth, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
};