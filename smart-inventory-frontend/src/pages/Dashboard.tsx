import React, { useEffect, useState } from 'react';
import ProductsPage from './ProductsPage';
import { getProductsTop, getCategories } from '../api/inventoryApi';

interface ProductSummary {
    id: string;
    name: string;
    quantity: number;
}

interface Category {
    id: string;
    name: string;
}

const Dashboard: React.FC = () => {
    const [topProducts, setTopProducts] = useState<ProductSummary[]>([]);
    const [categories, setCategories] = useState<Category[]>([]);
    const [loadingTop, setLoadingTop] = useState(true);
    const [errorTop, setErrorTop] = useState<string | null>(null);

    // Fetch top 5 products
    useEffect(() => {
        const fetchTopProducts = async () => {
            setLoadingTop(true);
            try {
                const res = await getProductsTop(5);
                setTopProducts(res.data);
                setErrorTop(null);
            } catch (err: any) {
                setErrorTop(err.message || 'Failed to fetch top products');
            } finally {
                setLoadingTop(false);
            }
        };
        fetchTopProducts();
    }, []);

    // Fetch categories (for charts or filters)
    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const res = await getCategories();
                setCategories(res.data);
            } catch (err: any) {
                console.error('Failed to fetch categories:', err);
            }
        };
        fetchCategories();
    }, []);

    return (
        <div style={{ padding: '1rem' }}>
            <h1>Dashboard</h1>

            {/* Quick Summary */}
            <section style={{ marginBottom: '2rem' }}>
                <h2>Top 5 Products</h2>
                {loadingTop && <p>Loading top products...</p>}
                {errorTop && <p style={{ color: 'red' }}>Error: {errorTop}</p>}
                {!loadingTop && !errorTop && topProducts.length > 0 ? (
                    <ul>
                        {topProducts.map(p => (
                            <li key={p.id}>
                                {p.name} – Quantity: {p.quantity}
                            </li>
                        ))}
                    </ul>
                ) : (
                    !loadingTop && !errorTop && <p>No top products found.</p>
                )}
            </section>

            <hr />

            {/* Inventory Insights (example chart) */}
            <section style={{ marginBottom: '2rem' }}>
                <h2>Inventory by Category</h2>
                {categories.length === 0 ? (
                    <p>Loading categories...</p>
                ) : (
                    <ul>
                        {categories.map(c => (
                            <li key={c.id}>{c.name}</li>
                        ))}
                    </ul>
                )}
            </section>

            <hr />

            {/* Full Products Table */}
            <section>
                <h2>All Products</h2>
                <ProductsPage />
            </section>
        </div>
    );
};

export default Dashboard;