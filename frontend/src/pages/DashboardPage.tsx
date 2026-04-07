import { useState } from "react";
import { NavigationBar } from "../components/Navigation";
import { Dashboard } from "../components/Dashboard";

export default function DashboardPage(){
    const [activeScreen] = useState("/dashboard");
    const renderScreen = () =>{
        return <Dashboard />;
    }
    return (
        <div className="min-h-screen bg-background flex">
            <NavigationBar activeScreen={activeScreen} />
            <main className="flex-1 overflow-auto">
                {renderScreen()}
            </main>
        </div>);
}