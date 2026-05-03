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
    // Only redirect to login if:
    // 1. Status is 401 (Unauthorized)
    // 2. NOT currently on login/register pages (don't redirect during login attempts)
    // 3. User has a token (meaning they were authenticated but session expired)
    if (error.response?.status === 401) {
      const currentPath = window.location.pathname;
      const isAuthPage = currentPath === "/login" || currentPath === "/register";
      const hasToken = localStorage.getItem("token");

      if (!isAuthPage && hasToken) {
        localStorage.removeItem("token");
        window.location.href = "/login";
      }
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

export const notificationsAPI = {
  // Lấy tất cả thông báo của người dùng hiện tại (dựa vào JWT token)
  getAll: () => api.get("/notification"),

  // Lấy danh sách thông báo theo UserId có phân trang (dành cho trang xem tất cả thông báo)
  getByUser: (userId: string, page = 0) => 
    api.get(`/notification/user/${userId}/page/${page}`),

  // Lấy chi tiết 1 thông báo
  getById: (id: string) => api.get(`/notification/${id}`),

  // Tạo thông báo mới
  create: (data: {
    recipientId: string;
    actorId: string;
    type: number;
    content: string;
    relatedEntityType: string;
    relatedEntityId: string;
    createdAt: string;
  }) => api.post("/notification", data),

  // Đánh dấu thông báo đã đọc
  update: (id: string) => api.put(`/notification/${id}`),

  // Xóa thông báo
  delete: (id: string) => api.delete(`/notification/${id}`),
};

export const postsAPI = {
  getAll: () => api.get("/post"),
  getByUser: (userId: string, page = 0) => api.get(`/post/user/${userId}/page/${page}`),
  getLiked: () => api.get("/posts/me/likes"),
  create: (
    content: string,
    visibility: number = 0,
    media: { MediaUrl: string; MediaType: number }[] = []
  ) => {
    return api.post("/post", { Content: content, Visibility: visibility, Media: media, Hashtags: null });
  },
  like: (postId: string) => api.post(`/posts/${postId}/likes`),
  unlike: (postId: string) => api.delete(`/posts/${postId}/likes`),
  isLiked: (postId: string) => api.get(`/posts/${postId}/likes/me`),
  getLikeCount: (postId: string) => api.get(`/posts/${postId}/likes/count`),
  delete: (postId: string) => api.delete(`/post/${postId}`),
  share: (postId: string, comment?: string) => api.post(`/post/${postId}/share`, { Comment: comment }),
  getShares: (postId: string, page = 0) => api.get(`/post/${postId}/shares`, { params: { page } }),
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
  removeFriend: (friendshipId: string) => api.delete(`/friendship/${friendshipId}`),
};

export const reportsAPI = {
  create: (postId: string, reason: string) =>
    api.post("/reports", { PostId: postId, Reason: reason }),
  getMyReports: (page = 1, pageSize = 10) =>
    api.get("/reports/me", {
      params: {
        Page: page,
        PageSize: pageSize,
      },
    }),
};

export default api;