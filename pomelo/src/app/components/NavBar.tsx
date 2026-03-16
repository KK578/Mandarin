import { auth0 } from "@/lib/auth0";

const ThemeToggleItem: React.FC<{ value: string }> = ({ value }) => {
  return (
    <li>
      <input
        type="radio"
        name="theme-dropdown"
        className="theme-controller w-full btn btn-sm btn-block btn-ghost justify-start"
        aria-label={value}
        value={value}
      />
    </li>
  );
};

const ThemeToggle: React.FC = () => {
  return (
    <div className="dropdown">
      <div tabIndex={0} role="button" className="btn btn-primary m-1">
        Theme
      </div>
      <ul
        tabIndex={-1}
        className="dropdown-content bg-base-300 rounded-box z-1 w-20 p-2 shadow-2xl"
      >
        <ThemeToggleItem value="default" />
        <ThemeToggleItem value="light" />
        <ThemeToggleItem value="dark" />
      </ul>
    </div>
  );
};

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
      <div className="flex-1 flex-row">
        <a className="btn btn-ghost font-tlm text-tlm text-xl" href="/">
          <img
            src="/images/logo.png"
            alt="The Little Mandarin Logo"
            className="w-8 h-8 mr-2"
          />
          The Little Mandarin
        </a>
      </div>
      <div className="navbar-end gap-2">
        <ThemeToggle />
        <UserProfile />
      </div>
    </div>
  );
};
