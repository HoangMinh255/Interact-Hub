export interface User {
  id: string;
  username: string;
  email: string;
  avatarUrl?: string;
  followersCount: number;
}

export interface Post {
  id: string;
  content: string;
  visibility: number;
  createdAt: string;
  authorName: string;
  authorAvatar?: string;
  mediaUrls: string[];
  likesCount: number;
  commentCount: number;
  author?: User;
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