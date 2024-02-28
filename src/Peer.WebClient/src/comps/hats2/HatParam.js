import React from 'react'

const HatParam = ({children, match = false}) => {
    let backgroundColor = match ? "bg-indigo-400" : "bg-gray-100";
    let textColor = match ? "text-indigo-50" : "text-gray-500";

  return (
    <div className={`rounded-full ${backgroundColor} px-3 py-1 uppercase text-xs tracking-wider ${textColor} font-semibold shrink-0`}>
        {children}
    </div>
  )
}

export default HatParam