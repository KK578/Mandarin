import { auth0 } from "@/lib/auth0";

const UserProfile: React.FC = async () => {
  const session = await auth0.getSession();

  if (!session) {
    return <a href="/auth/login">Login</a>;
  }

  return (
    <div className="dropdown dropdown-end">
      <div
        tabIndex={0}
        role="button"
        className="btn btn-ghost btn-circle avatar"
      >
        <div className="w-10 rounded-full">
          <img alt="User Avatar" src={session.user.picture} />
        </div>
      </div>
      <ul
        tabIndex={-1}
        className="menu menu-sm dropdown-content bg-base-300 rounded-box z-1 mt-3 w-48 p-2 shadow"
      >
        <li>
          <h3>Hi, {session.user.name}!</h3>
        </li>
        <li>
          <a href="/auth/logout">Logout</a>
        </li>
      </ul>
    </div>
  );
};

export const NavBar: React.FC = () => {
  return (
    <div className="navbar bg-base-100 shadow-sm">
      <div className="flex-1">
        <a className="btn btn-ghost text-xl" href="/">
          The Little Mandarin
        </a>
      </div>
      <div className="flex-none">
        <UserProfile />
      </div>
    </div>
  );
};
