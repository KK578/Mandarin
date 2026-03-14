export const AuthGate: React.FC = async () => {
  return (
    <div className="hero h-full bg-base-200">
      <div className="hero-content text-center">
        <div className="max-w-md">
          <h1 className="text-5xl font-bold">Hello there</h1>
          <p className="py-6">You don't seem to be logged in yet!</p>
          <a href="/auth/login">
            <button className="btn btn-primary">Login</button>
          </a>
        </div>
      </div>
    </div>
  );
};
