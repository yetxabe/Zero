import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { jwtDecode } from "jwt-decode";
import { extractEmailFromJwtPayload, extractRolesFromJwtPayload} from "../helpers/claims.js";
import { me } from "../api/auth";

const AuthContext = createContext(null);

export function AuthProvider({ children}) {
    const [token, setToken] = useState(()=>localStorage.getItem('access_token'));
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    //Refrescar usuario al cargar si hay token
    useEffect(()=>{
        (async () => {
            if(!token){
                setLoading(false);
                return;
            }
            try{
                const payload = jwtDecode(token);
                const roles = extractRolesFromJwtPayload(payload);
                const email = extractEmailFromJwtPayload(payload);
                setUser({email, roles});
            } catch(e){
                console.error("Token invalido", e);
                localStorage.removeItem('access_token');
                setToken(null);
            }finally{
                setLoading(false);
            }
        })();
    },[token]);

    const loginAction = (newToken, maybeUser) => {
        localStorage.setItem('access_token', newToken);
        setToken(newToken);
        if(maybeUser) setUser(maybeUser);
    };

    const logout = () => {
        localStorage.removeItem('access_token');
        setToken(null);
        setUser(null);
    }

    const value = useMemo(() => ({token, user, loading, loginAction, logout}), [token, user, loading]);

    return(
      <AuthContext.Provider value={value}>
          {children}
      </AuthContext.Provider>
    );
}

export function useAuth(){
    const ctx = useContext(AuthContext);
    if(!ctx) throw new Error("useAuth debe ser usado dentro de un AuthProvider");
    return ctx;
}