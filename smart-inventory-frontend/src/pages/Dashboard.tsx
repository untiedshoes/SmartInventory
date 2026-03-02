import React, { useEffect, useState } from 'react';
import { getProducts } from '../api/inventoryApi';

interface Product {
    id: number;
    name: string;
    quantity: number;
}

const Dashboard: React.FC = () => {
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const response = await getProducts();
                setProducts(response.data);
            } catch (err: any) {
                setError(err.message || 'Failed to fetch products');
            } finally {
                setLoading(false);
            }
        };

        fetchProducts();
    }, []);

    if (loading) return <p>Loading products...</p>;
    if (error) return <p>Error: {error}</p>;

    return (
        <div style={{ padding: '20px' }}>
            <h2>Dashboard</h2>
            <ul>
                {products.map(p => (
                    <li key={p.id}>{p.name} – Quantity: {p.quantity}</li>
                ))}
            </ul>
        </div>
    );
};

export default Dashboard;