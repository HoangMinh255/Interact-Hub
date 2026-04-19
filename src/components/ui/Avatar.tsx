interface AvatarProps {
  name: string;
  size?: "sm" | "md" | "lg";
  color?: string;
}

const sizeMap = {
  sm: "w-8 h-8 text-xs",
  md: "w-10 h-10 text-sm",
  lg: "w-14 h-14 text-lg",
};

const Avatar = ({ name, size = "md", color = "bg-blue-500" }: AvatarProps) => {
  return (
    <div className={`${sizeMap[size]} ${color} rounded-full flex items-center justify-center text-white font-medium flex-shrink-0`}>
      {name.charAt(0).toUpperCase()}
    </div>
  );
};

export default Avatar;