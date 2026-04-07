"use client";
import {
  LayoutDashboard,
  Bell,
  Settings,
  LogOut,
} from "lucide-react";
import { useState, useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";
import { Button } from "./ui/button";

interface Navigation{
    activeScreen : string;
}

const navItems = [
    { id: "Dashboard", label: "Trang chủ", icon: LayoutDashboard, href: "/dashboard"}, 
    { id: "Notification", label: "Thông báo", icon: Bell, href: "/notification"},
    { id: "Settings", label: "Cài đặt thông tin", icon: Settings, href: "/settings"},
    
];

export function NavigationBar({activeScreen} : Navigation){
    const [user, setUser] = useState<any>(null);
    const [navigationItems, setNavigationItems] =useState<any>([]);
    const router = useNavigate();

    useEffect(() => {
        const storeUser = localStorage.getItem("user");
        if (storeUser) {
            const parseUser = JSON.parse(storeUser);
            // eslint-disable-next-line react-hooks/set-state-in-effect
            setUser(parseUser);
            setNavigationItems(navItems);
        }
        // else{
        //     router("/login");
        // }
    }, [router]);

    const handleClickLogout = () =>{
        setUser(null);
        router("/login");
    }

    return(
        <>
            <div className="w-64 bg-sidebar border-r border-sidebar-border h-screen flex flex-col border-[#e5e5e5]">
                <div className="p-6 border-b border-sidebar-border border-[#e5e5e5]">
                    <div className="flex items-center gap-3">
                        <h2 className="font-bold">Interact Hub</h2>
                    </div>
                </div>
                <nav className="flex-1 p-4 space-y-2 overflow-y-auto">
                    {navigationItems.map((item : any) => {
                        const Icon=item.icon;
                        return(
                            <div className="mr-5">
                                <Link to={item.href} key={item.id}>
                                    <Button 
                                        variant={activeScreen === item.id ? "default" : "ghost" }
                                        className={
                                            activeScreen === item.href
                                                ? "w-full justify-start h-11 bg-[#2563eb] border border-[#2563eb] text-[#FFFFFF]"
                                                : "w-full justify-start h-11 bg-[#f8fafc] border border-[#f8fafc] hover:bg-[#e0e7ff] hover:text-[#1e3a8a] hover:border-[#c7d2fe] hover:border-3 cursor-pointer"
                                        }
                                    >   
                                        <Icon className="w-5 h-5 mr-3" />
                                        {item.label}
                                    </Button>
                                </Link>
                            </div>
                        )
                    })}
                </nav>
                <div className="p-1 border-t border-sidebar-border border-[#e5e5e5]">
                    <div className="relative flex items-center gap-3 p-3 bg-accent rounded-lg">
                    <div className="relative w-12 h-12 rounded-full overflow-hidden bg-[#e3f5f0]">
                        <img
                        src={"/imgs/" + (user?.profilePicture || "default-avatar.jpeg")}
                        alt="User Avatar"
                        className="h-full w-full object-cover"
                        />
                    </div>
                    <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium truncate">
                        {user?.name || "admin"}
                        </p>
                        <p className="text-xs text-muted-foreground">
                        {user?.rolename || "admin"}
                        </p>
                    </div>
                    <div>
                        <p className="text-xs text-muted-foreground">
                        <Button
                            className="bg-[#e74c3c] border-0 text-white  hover:bg-[#c0392b]  roundef-full cursor-pointer"
                            onClick={handleClickLogout}
                        >
                            <LogOut />
                        </Button>
                        </p>
                    </div>
                    </div>
                </div>
            </div>            
            
            
        </>
    );
}
