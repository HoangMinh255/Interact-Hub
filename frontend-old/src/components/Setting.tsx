import { Card, CardHeader, CardTitle } from "./ui/card";


export function Setting() {

  const user=localStorage.getItem('user');

  console.log(user);

  return (
    <div className="p-6 space-y-6 bg-[#FFFFFF]">      
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <Card className="lg:col-span-2">
          <CardHeader className="flex flex-row items-center justify-between">
                
          </CardHeader>
        </Card>

      </div>
    </div>
  );
}