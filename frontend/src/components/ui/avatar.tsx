import { resolveMediaUrl } from "../../api";

interface AvatarProps {
  name: string;
  avatarUrl?: string | null;
  size?: "sm" | "md" | "lg" | "xl";
  color?: string;
}

const sizeMap = {
  sm: "w-8 h-8 text-xs",
  md: "w-10 h-10 text-sm",
  lg: "w-14 h-14 text-lg",
  xl: "w-20 h-20 text-2xl",
};

const Avatar = ({ name, avatarUrl, size = "md", color = "bg-blue-500" }: AvatarProps) => {
  const resolved = resolveMediaUrl(avatarUrl ?? undefined);

  const baseClasses = `${sizeMap[size]} rounded-full flex items-center justify-center text-white font-medium flex-shrink-0`;

  if (resolved) {
    return (
      // eslint-disable-next-line jsx-a11y/alt-text
      <img src={resolved} className={`${baseClasses} object-cover`} referrerPolicy="no-referrer" />
    );
  }

  return (
    <div className={`${baseClasses} ${color}`}>
      {name.charAt(0).toUpperCase()}
    </div>
  );
};

export default Avatar;