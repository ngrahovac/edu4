import React from 'react'
import { useState, useEffect } from 'react';

const SentApplicationsSorter = (props) => {
    const {
        onSortSelected
    } = props;

    const [selectedSort, setSelectedSort] = useState("Default");

    const handleSelectChange = (e) => {
        setSelectedSort(e.target.value);
    };

    useEffect(() => {
      onSortSelected(selectedSort);
    }, [selectedSort])
    

    return (
        <div className='flex flex-col space-y-2 w-64'>
            <p>Sort by:</p>
            <select value={selectedSort} onChange={handleSelectChange} className='rounded-xl'>
                <option value="Default">Default</option>
                <option value="OldestFirst">Oldest first</option>
                <option value="NewestFirst">Newest first</option>
            </select>
        </div>
    )
}

export default SentApplicationsSorter