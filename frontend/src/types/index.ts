export interface User {
  id: string;
  username: string;
  email: string;
  avatarUrl?: string;
  followersCount: number;
}

export interface Post {
  id: string;
  author: User;
  content: string;
  imageUrl?: string;
  likesCount: number;
  commentsCount: number;
  createdAt: string;
}

export interface Notification {
  id: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
}

export interface Hashtag {
  tag: string;
  count: number;
}