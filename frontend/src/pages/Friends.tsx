import Sidebar from "../components/layout/Sidebar";

const friends = [
  { id: "1", name: "Alice Johnson", mutual: 5 },
  { id: "2", name: "David Brown", mutual: 3 },
  { id: "3", name: "Eve Davis", mutual: 8 },
  { id: "4", name: "Carol White", mutual: 2 },
  { id: "5", name: "Bob Smith", mutual: 6 },
];

const Friends = () => {
  return (
    <div className="max-w-5xl mx-auto px-4 py-4 flex gap-4">
      <Sidebar />
      <div className="max-w-3xl mx-auto px-4 py-6">
        <div className="bg-white border border-gray-200 rounded-xl overflow-hidden">
          <div className="px-6 py-4 border-b border-gray-100">
            <h2 className="text-base font-medium text-gray-800">Gợi ý kết bạn</h2>
          </div>

          <div className="divide-y divide-gray-100">
            {friends.map((friend) => (
              <div key={friend.id} className="flex items-center gap-4 px-6 py-4 hover:bg-gray-50">
                <div className="w-12 h-12 rounded-full bg-blue-400 flex items-center justify-center text-white font-medium">
                  {friend.name.charAt(0)}
                </div>
                <div className="flex-1">
                  <p className="text-sm font-medium text-gray-800">{friend.name}</p>
                  <p className="text-xs text-gray-400">{friend.mutual} bạn chung</p>
                </div>
                <div className="flex gap-2">
                  <button className="px-4 py-1.5 bg-blue-500 text-white text-sm rounded-lg hover:bg-blue-600">
                    Kết bạn
                  </button>
                  <button className="px-4 py-1.5 bg-gray-100 text-gray-600 text-sm rounded-lg hover:bg-gray-200">
                    Bỏ qua
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Friends;