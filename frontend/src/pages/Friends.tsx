import Sidebar from "../components/layout/Sidebar";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { friendsAPI } from "../api";
import Avatar from "../components/ui/avatar";

const Friends = () => {
  const { user } = useAuth();
  const [items, setItems] = useState<Array<any>>([]);
  const [loading, setLoading] = useState(false);
  const [view, setView] = useState<"requests" | "friends" | "suggestions">("requests");
  const [page, setPage] = useState(0);
  const [totalPages, setTotalPages] = useState(0);

  useEffect(() => {
    if (!user?.id) return;
    loadView(page);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user?.id, view, page]);

  const loadView = async (pageNumber = 0) => {
    try {
      setLoading(true);
      let res: any;
      if (view === "requests") {
        res = await friendsAPI.getRequest(user!.id, pageNumber);
      } else if (view === "friends") {
        res = await friendsAPI.getFriendsList(user!.id, pageNumber);
      } else {
        res = await friendsAPI.getSuggestions(user!.id, pageNumber);
      }

      const payload = res.data?.data ?? res.data ?? [];
      const nextItems = Array.isArray(payload) ? payload : payload.items ?? [];
      setItems(nextItems);
      setTotalPages(typeof payload?.totalPages === "number" ? payload.totalPages : 0);
    } catch (err) {
      console.error("Failed to load friendship view", err);
      setItems([]);
      setTotalPages(0);
    } finally {
      setLoading(false);
    }
  };

  const handleAccept = async (friendshipId: string) => {
    try {
      await friendsAPI.acceptRequest(friendshipId);
      await loadView(page);
    } catch (err) {
      console.error("Failed to accept request", err);
    }
  };

  const handleReject = async (friendshipId: string) => {
    try {
      await friendsAPI.rejectRequest(friendshipId);
      await loadView(page);
    } catch (err) {
      console.error("Failed to reject request", err);
    }
  };

  const handleSendRequest = async (targetId: string) => {
    if (!user?.id) return;
    try {
      setLoading(true);
      await friendsAPI.sendRequest(user.id, targetId);
      await loadView(page);
    } catch (err) {
      console.error("Failed to send friend request", err);
    } finally {
      setLoading(false);
    }
  };

  const handleRemoveFriend = async (targetId: string) => {
    if (!user?.id) return;
    try {
      setLoading(true);
      await friendsAPI.removeFriend(targetId);
      await loadView(page);
    } catch (err) {
      console.error("Failed to remove friend", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-5xl mx-auto px-4 py-4 flex gap-4">
      <div className="hidden md:block">
        <Sidebar />
      </div>
      <div className="flex-1 px-0 md:px-4 py-6">
        <div className="bg-white border border-gray-200 rounded-xl overflow-hidden">
          <div className="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
            <div className="flex gap-2">
              <button
                onClick={() => { setView("requests"); setPage(0); }}
                className={"px-3 py-1 rounded-lg text-sm " + (view === "requests" ? "bg-blue-500 text-white" : "bg-gray-100 text-gray-700")}
              >
                Lời mời
              </button>
              <button
                onClick={() => { setView("friends"); setPage(0); }}
                className={"px-3 py-1 rounded-lg text-sm " + (view === "friends" ? "bg-blue-500 text-white" : "bg-gray-100 text-gray-700")}
              >
                Bạn bè
              </button>
              <button
                onClick={() => { setView("suggestions"); setPage(0); }}
                className={"px-3 py-1 rounded-lg text-sm " + (view === "suggestions" ? "bg-blue-500 text-white" : "bg-gray-100 text-gray-700")}
              >
                Gợi ý
              </button>
            </div>
          </div>

          <div className="divide-y divide-gray-100">
              {loading && <div className="p-4 text-sm text-gray-500">Đang tải...</div>}
              {!loading && items.length === 0 && (
                <div className="p-4 text-sm text-gray-500">Không có kết quả.</div>
              )}

              {items.map((r: any) => {
                const key = r.friendshipId ?? r.friendId ?? r.id;
                const targetId = r.friendId ?? r.id ?? r.userId ?? r.targetId;
                const name = r.friendName ?? r.fullName ?? r.userName ?? r.name ?? "Người dùng";
                return (
                  <div key={key} className="flex items-center gap-4 px-6 py-4 hover:bg-gray-50">
                    <Link to={targetId ? `/profile/${targetId}` : `/profile`} className="shrink-0">
                      <Avatar name={name} size="md" />
                    </Link>
                    <div className="flex-1">
                      {targetId ? (
                        <Link to={`/profile/${targetId}`} className="text-sm font-medium text-gray-800 hover:underline">{name}</Link>
                      ) : (
                        <p className="text-sm font-medium text-gray-800">{name}</p>
                      )}
                    </div>
                    <div className="flex flex-col sm:flex-row gap-2">
                      {view === "requests" && (
                        <>
                          <button onClick={() => handleAccept(r.friendshipId)} className="px-4 py-1.5 bg-blue-500 text-white rounded">Chấp nhận</button>
                          <button onClick={() => handleReject(r.friendshipId)} className="px-4 py-1.5 bg-gray-100 rounded">Từ chối</button>
                        </>
                      )}
                      {view === "suggestions" && (
                        <button onClick={() => handleSendRequest(r.friendId ?? r.id)} className="px-4 py-1.5 bg-blue-500 text-white rounded">Kết bạn</button>
                      )}
                      {view === "friends" && (
                        <button onClick={() => handleRemoveFriend(r.friendId ?? r.id)} className="px-4 py-1.5 bg-gray-100 rounded">Bỏ bạn</button>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>

          {totalPages > 0 && (
            <div className="px-6 py-4 border-t border-gray-100 flex items-center justify-between">
              <button
                onClick={() => setPage((current) => Math.max(current - 1, 0))}
                disabled={page === 0 || loading}
                className="px-3 py-1.5 rounded-lg text-sm bg-gray-100 text-gray-700 disabled:opacity-50"
              >
                Trước
              </button>
              <span className="text-sm text-gray-500">
                Trang {page + 1} / {totalPages}
              </span>
              <button
                onClick={() => setPage((current) => current + 1)}
                disabled={page + 1 >= totalPages || loading}
                className="px-3 py-1.5 rounded-lg text-sm bg-gray-100 text-gray-700 disabled:opacity-50"
              >
                Sau
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Friends;