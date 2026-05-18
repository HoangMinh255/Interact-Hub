import { useState, useEffect } from "react";
import { postsAPI } from "../api";
import type { Post } from "../types";

type PostApiResponse = {
  id: string;
  originalAuthorId?: string;
  visibility?: number;
  originalContent: string;
  createdAt: string;
  originalAuthorName?: string;
  originalAuthorAvatar?: string;
  mediaUrls?: string[];
  commentCount?: number;
  isShared?: boolean;
  content?: string;
  authorId?: string;
  authorName?: string;
  authorAvatar?: string | null;
  originalPostId?: string;
  hashtag?: string[];
};

const mapPost = (post: PostApiResponse): Post => ({
  id: post.id,
  visibility: post.visibility ?? 0,
  originalAuthorName: post.originalAuthorName ?? "Unknown",
  originalAuthorAvatar: post.originalAuthorAvatar ?? "",
  originalAuthorId: post.originalAuthorId,
  originalContent: post.originalContent,
  mediaUrls: post.mediaUrls??[],
  likesCount: 0,
  commentCount: post.commentCount ?? 0,
  createdAt: new Date(post.createdAt).toLocaleString("vi-VN"),
  isShared: post.isShared ?? false,
  content: post.content ?? "",
  authorId: post.authorId ?? "",
  authorName: post.authorName ?? "Unknown",
  authorAvatar: post.authorAvatar ?? undefined,
  originalPostId: post.originalPostId,
  hashtag: post.hashtag ?? [],
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