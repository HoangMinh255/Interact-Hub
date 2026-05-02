import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5226/api",
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
  register: (userName: string, fullName: string, email: string, password: string) =>
    api.post("/auth/register", { userName, fullName, email, password }),
};

export const postsAPI = {
  getAll: () => api.get("/post"),
  create: (content: string, media: { MediaUrl: string; MediaType: number }[] = []) => {
    return api.post("/post", { Content: content, Visibility: 0, Media: media, Hashtags: null });
  },
  like: (postId: string) => api.post(`/post/${postId}/like`),
  delete: (postId: string) => api.delete(`/post/${postId}`),
};

export const commentsAPI = {
  getByPostId: (postId: string, page = 0) =>
    api.get(`/comment/post/${postId}/page/${page}`),
  create: (postId: string, content: string, parentCommentId?: string | null) =>
    api.post("/comment", { PostId: postId, Content: content, ParentCommentId: parentCommentId }),
};

export const usersAPI = {
  getProfile: (userId: string) => api.get(`/users/${userId}`),
  updateProfile: (data: object) => api.put("/users/profile", data),
};

export const friendsAPI = {
  getSuggestions: () => api.get("/friendship"), //get friend suggestions (?)
  sendRequest: (requesterId: string, receiverId: string) => api.post("/friendship", { requesterId, receiverId }), 
  getRequest: (userId: string, page: number) => api.get(`/friendship/friendRequests/${userId}/${page}`),
  getFriendsList: (userId: string, page: number) => api.get(`/friendship/friends/${userId}/${page}`),
  acceptRequest: (requesterId: string) => api.put(`/friendship/accept/${requesterId}`),
  blockRequest: (userId: string, targetId: string) => api.put(`/friendship/block/${userId}/${targetId}`),
  rejectRequest: (requesterId: string) => api.delete(`/friendship/reject/${requesterId}`),
  removeFriend: (targetId: string) => api.delete(`/friendship/${targetId}`),
};

export default api;