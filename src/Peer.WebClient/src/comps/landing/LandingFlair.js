import React from 'react'

const LandingFlair = (props) => {
    const {
        children,
        selected
    } = props;

    return (
        <div className={`cursor-pointer flex shrink-0 rounded-full font-semibold ${selected? 'bg-indigo-400 text-white' : 'bg-indigo-100 text-gray-300'} px-3 py-1 shrink-0 content-center`}>
            {children}
        </div>
    )
}

export default LandingFlair