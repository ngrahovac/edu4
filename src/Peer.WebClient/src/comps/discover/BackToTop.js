import React from 'react'

const BackToTop = ({onClick}) => {
    return (
        <button 
            onClick={onClick}
            className='w-12 h-12 rounded-full flex items-center place-content-center drop-shadow-sm shadow-sm text-gray-700 bg-gray-100'>
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                <path strokeLinecap="round" strokeLinejoin="round" d="M4.5 10.5 12 3m0 0 7.5 7.5M12 3v18" />
            </svg>
        </button>
    )
}

export default BackToTop