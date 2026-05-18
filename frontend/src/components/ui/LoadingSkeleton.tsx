const LoadingSkeleton = () => {
  return (
    <div className="bg-white border border-gray-200 rounded-xl p-4 animate-pulse">
      <div className="flex items-center gap-3 mb-3">
        <div className="w-9 h-9 rounded-full bg-gray-200"></div>
        <div className="flex-1">
          <div className="h-3 bg-gray-200 rounded w-1/3 mb-2"></div>
          <div className="h-2 bg-gray-200 rounded w-1/4"></div>
        </div>
      </div>
      <div className="h-3 bg-gray-200 rounded w-full mb-2"></div>
      <div className="h-3 bg-gray-200 rounded w-4/5 mb-2"></div>
      <div className="h-3 bg-gray-200 rounded w-3/5"></div>
    </div>
  );
};

export default LoadingSkeleton;