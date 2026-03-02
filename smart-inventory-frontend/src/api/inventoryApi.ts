import axios from "axios";

const api = axios.create({
    baseURL: 'http://localhost:5100/api',
});

export const getAll = async () => api.get('/products');
export const getProducts = async () => api.get('/products');
export const createProduct = async (product: any) => api.post('/products', product);