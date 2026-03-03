import React from "react";
import { useParams } from "react-router-dom";

const ItemDetail: React.FC = () => {
    const { id } = useParams<{ id: string }>();

    return (
        <div style={{ padding: '0' }}>
            <h2>Item Detail</h2>
            <p>Viewing details for item ID: {id}</p>
        </div>
    );
};

export default ItemDetail;