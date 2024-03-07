import React from 'react'
import { useState, useEffect } from 'react';

const ApplicationsSorter = (props) => {
    const {
        onSortSelected,
        sort = undefined
    } = props;

    const [selectedSort, setSelectedSort] = useState(sort ? sort : "Default");

    const handleSelectChange = (e) => {
        setSelectedSort(e.target.value);
    };

    useEffect(() => {
        onSortSelected(selectedSort);
    }, [selectedSort])


    return (
        <select
            value={selectedSort}
            onChange={handleSelectChange}
            className='rounded-full border-gray-200 text-gray-700 text-base'>
            <option value="Default">Default sort</option>
            <option value="OldestFirst">Oldest sent first</option>
            <option value="NewestFirst">Recently sent first</option>
        </select>
    )
}

export default ApplicationsSorter