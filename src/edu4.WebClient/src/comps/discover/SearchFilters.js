import React from 'react'

const SearchFilters = ({ children }) => {
    
    return (
        <div className='flex flex-row flex-wrap mt-4 space-x-1'>
            {children}
        </div>
    )
}

export default SearchFilters