import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5000/api",
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export const authAPI = {
  login: (email: string, password: string) =>
    api.post("/auth/login", { email, password }),
  register: (username: string, email: string, password: string) =>
    api.post("/auth/register", { username, email, password }),
};

export const postsAPI = {
  getAll: () => api.get("/posts"),
  create: (content: string, imageUrl?: string) =>
    api.post("/posts", { content, imageUrl }),
  like: (postId: string) => api.post(`/posts/${postId}/like`),
  delete: (postId: string) => api.delete(`/posts/${postId}`),
};

export const usersAPI = {
  getProfile: (userId: string) => api.get(`/users/${userId}`),
  updateProfile: (data: object) => api.put("/users/profile", data),
};

export const friendsAPI = {
  getSuggestions: () => api.get("/friends/suggestions"),
  sendRequest: (userId: string) => api.post(`/friends/request/${userId}`),
  acceptRequest: (userId: string) => api.put(`/friends/accept/${userId}`),
};

export default api;