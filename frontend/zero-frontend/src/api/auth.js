import api from './axios';

export const login = async({email, password}) => {
   const {data} = await api.post('/auth/login', {email, password});
   return data;
};

export const register = async({email, password, firstName, lastName, izaroCode}) => {
    const {data} = await api.post('/auth/register', {
        email,
        password,
        firstName,
        lastName,
        izaroCode
    });
    return data;
};

export const me = async() => {
    const {data} = await api.get('/auth/me');
    return data;
}