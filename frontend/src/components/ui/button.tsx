interface ButtonProps {
  label: string;
  onClick?: () => void;
  type?: "button" | "submit";
  variant?: "primary" | "secondary" | "danger";
  fullWidth?: boolean;
}

const variantMap = {
  primary: "bg-blue-500 text-white hover:bg-blue-600",
  secondary: "bg-gray-100 text-gray-600 hover:bg-gray-200",
  danger: "bg-red-500 text-white hover:bg-red-600",
};

function Button({ label, onClick, type = "button", variant = "primary", fullWidth = false }: ButtonProps) {
  return (
    <button
      type={type}
      onClick={onClick}
      className={`h-10 px-4 rounded-lg text-sm font-medium ${variantMap[variant]} ${fullWidth ? "w-full" : ""}`}
    >
      {label}
    </button>
  );
}

export default Button;