import { useState, useEffect } from "react";
import { postsAPI } from "../api";
import type { Post } from "../types";

type PostApiResponse = {
  id: string;
  authorId?: string;
  visibility?: number;
  content: string;
  createdAt: string;
  authorName?: string;
  authorAvatar?: string;
  mediaUrls?: string[];
  commentCount?: number;
};

const mapPost = (post: PostApiResponse): Post => ({
  id: post.id,
  visibility: post.visibility ?? 0,
  authorName: post.authorName ?? "Unknown",
  authorAvatar: post.authorAvatar ?? "",
  author: post.authorId ? { id: post.authorId, fullName: post.authorName ?? "", userName: "", email: "", followersCount: 0 } : undefined,
  content: post.content,
  mediaUrls: post.mediaUrls??[],
  likesCount: 0,
  commentCount: post.commentCount ?? 0,
  createdAt: new Date(post.createdAt).toLocaleString("vi-VN"),
});

export const usePosts = () => {
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(true);

  const loadPosts = async () => {
    setLoading(true);
    try {
      const response = await postsAPI.getAll();
      const normalizedPosts = Array.isArray(response.data)
        ? response.data.map(mapPost)
        : [];
      setPosts(normalizedPosts);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadPosts();
  }, []);

  return { posts, loading, reloadPosts: loadPosts, setPosts };
};