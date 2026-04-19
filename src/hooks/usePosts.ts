import { useState, useEffect } from "react";
import type { Post } from "../types";

export const usePosts = () => {
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setTimeout(() => {
      setPosts([]);
      setLoading(false);
    }, 500);
  }, []);

  return { posts, loading };
};