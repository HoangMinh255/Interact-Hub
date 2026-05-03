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
  register: (username: string, fullName: string, email: string, password: string) =>
    api.post("/auth/register", { username, fullName, email, password }),
  me: () => api.get("/auth/me"),
};

export const postsAPI = {
  getAll: () => api.get("/post"),
  getByUser: (userId: string, page = 0) => api.get(`/post/user/${userId}/page/${page}`),
  create: (
    content: string,
    visibility: number = 0,
    media: { MediaUrl: string; MediaType: number }[] = []
  ) => {
    return api.post("/post", { Content: content, Visibility: visibility, Media: media, Hashtags: null });
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
  getUser: () => api.get(`/users/me`),
  getUserById: (userId: string) => api.get(`/users/${userId}`),
  searchUsers: (keyword: string, page = 1, pageSize = 10) =>
    api.get(`/users/search`, {
      params: {
        Keyword: keyword,
        Page: page,
        PageSize: pageSize,
      },
    }),
  updateProfile: (fullname: string, bio: string) => api.put("/users/me", { FullName: fullname, Bio: bio }),
  uploadAvatar: (avatarFile: File) => {
    const formData = new FormData();
    formData.append("AvatarFile", avatarFile);
    return api.post("/users/me/avatar", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  },
};

export const storiesAPI = {
  getMyStories: () => api.get("/stories/me"),
};

export const friendsAPI = {
  getSuggestions: (userId: string, page = 0) => api.get(`/friendship/suggestion/${userId}/${page}`),
  sendRequest: (requesterId: string, receiverId: string) => api.post("/friendship", { requesterId, receiverId }), 
  getRequest: (userId: string, page: number) => api.get(`/friendship/friendRequests/${userId}/${page}`),
  getFriendsList: (userId: string, page: number) => api.get(`/friendship/friends/${userId}/${page}`),
  acceptRequest: (requesterId: string) => api.put(`/friendship/accept/${requesterId}`),
  blockRequest: (userId: string, targetId: string) => api.put(`/friendship/block/${userId}/${targetId}`),
  rejectRequest: (requesterId: string) => api.delete(`/friendship/reject/${requesterId}`),
  removeFriend: (targetId: string) => api.delete(`/friendship/${targetId}`),
};

export default api;