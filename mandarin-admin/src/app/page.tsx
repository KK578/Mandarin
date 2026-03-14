import { auth0 } from "@/lib/auth0";
import { AuthGate } from "./components/AuthGate";

export default async function Home() {
  const session = await auth0.getSession();

  if (!session) {
    return <AuthGate />;
  }

  return (
    <div className="hero h-full bg-base-200">
      <div className="hero-content text-center">
        <div className="max-w-md">
          <h1 className="text-5xl font-bold">Hello there</h1>
          <p className="py-6">
            Welcome to the new admin app for The Little Mandarin. This is
            currently a placeholder.
          </p>
        </div>
      </div>
    </div>
  );
}
