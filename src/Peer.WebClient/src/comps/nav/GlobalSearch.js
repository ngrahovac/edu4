import React from 'react'
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const GlobalSearch = () => {
    const [keyword, setKeyword] = useState('');
    const [isTyping, setIsTyping] = useState(false);

    const navigate = useNavigate();

    function handleKeywordChange(e) {
        setKeyword(e.target.value);
    }

    function handleKeyDown(e) {
        if (e.key == "Enter") {
            navigate(`/search?keyword=${keyword}`)
        }
    }

    return (
        <div className='flex items-center justify-between bg-white px-4 gap-x-2 rounded-full'>
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="lightGray" className="w-5 h-5">
                <path strokeLinecap="round" strokeLinejoin="round" d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z" />
            </svg>

            <input
                type="text"
                name="keyword"
                placeholder="Search projects..."
                onChange={handleKeywordChange}
                maxLength={100}
                value={keyword}
                onFocus={() => setIsTyping(true)}
                onBlur={() => setIsTyping(false)}
                onKeyDown={handleKeyDown}
                className="rounded-full border-none focus:border-none focus:ring-0">
            </input>


            <span className={`text-gray-300 font-semibold ${isTyping ? '' : 'invisible'}`}>Enter</span>
        </div>
    )
}

export default GlobalSearch