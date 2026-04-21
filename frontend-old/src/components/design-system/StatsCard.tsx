import { Card, CardContent } from '../ui/card';
import { cn } from '../ui/utils';


interface StatsCardProps {
  title: string;
  value: string | number;
  subtitle?: string;
  icon?: |" LucideIcon";
  trend?: {
    value: number;
    label: string;
    direction: 'up' | 'down' | 'neutral';
  };
  className?: string;
  variant?: 'default' | 'success' | 'warning' | 'danger';
}

const variantStyles = {
  default: {
    card: 'border-border',
    icon: 'text-primary bg-primary/10',
    value: 'text-foreground'
  },
  success: {
    card: 'border-green-200 bg-green-50/50',
    icon: 'text-green-600 bg-green-100',
    value: 'text-green-900'
  },
  warning: {
    card: 'border-orange-200 bg-orange-50/50',
    icon: 'text-orange-600 bg-orange-100',
    value: 'text-orange-900'
  },
  danger: {
    card: 'border-red-200 bg-red-50/50',
    icon: 'text-red-600 bg-red-100',
    value: 'text-red-900'
  }
};

export function StatsCard({ 
  title, 
  value, 
  subtitle, 
  icon: Icon, 
  trend,
  className,
  variant = 'default'
}: StatsCardProps) {
  const styles = variantStyles[variant];

  return (
    <Card className={cn(
      'transition-all duration-200 hover:shadow-md',
      styles.card,
      className
    )}>

    </Card>
  );
}