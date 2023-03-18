import { Route } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { Navigate } from "react-router-dom";

interface Props {
	component: React.ComponentType,
}

export function PublicRoute({ component: Component } : Props) : JSX.Element{
	const { isAuthenticate } = useAuth();

	return !isAuthenticate() ? <Component /> : (
		<Navigate to="/" />
	)
}