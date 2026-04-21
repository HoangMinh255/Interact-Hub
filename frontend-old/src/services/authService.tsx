import api from "../lib/apiFetch";

export async function Login(username: string, password: string) {
    const response = await api.post("/auth/login", { username, password });
    return response.data;
}   