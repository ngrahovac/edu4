import React from 'react'

const Search = () => {
    return (
        <label className="relative block z-10">
            <input className="h-9 w-64 pr-4 pl-9 bg-white border border-slate-300 rounded-md focus:outline-none focus:border-blue-500 focus:blue-500 text-sm text-slate-700 placeholder:italic placeholder:text-slate-400 " placeholder="Search" type="text" name="search" />
            <span className="absolute inset-y-0 flex items-center left-2">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth="1.5" className="w-5 h-5 stroke-slate-400">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
                </svg>
            </span>
        </label>
    )
}

export default Search