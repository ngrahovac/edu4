import React from 'react'
import { Link } from 'react-router-dom'

const LogoIcon = () => {
    return (
        <Link to="/welcome">
            <div className='bg-indigo-600 rounded-full w-8 h-8'>
            </div>
        </Link>
    )
}

export default LogoIcon