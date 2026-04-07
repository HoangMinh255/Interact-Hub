import { useState } from "react";
import { NavigationBar } from "../components/Navigation";
import { Setting } from "../components/Setting";

export default function SettingPage(){
    const [activeScreen] = useState("/setting");
    const renderScreen = () =>{
        return <Setting />;
    }
    return (
        <div className="min-h-screen bg-background flex">
            <NavigationBar activeScreen={activeScreen} />
            <main className="flex-1 overflow-auto">
                {renderScreen()}
            </main>
        </div>);
}