import React from 'react'
import { Link } from 'react-router-dom';

const TopNavbarItem = (props) => {
    const {
        text,
        icon,
        link
    } = props;

    return (
        <Link to={link}>
            <div className="flex shrink-0">
                <button className="flex items-center text-slate-700 font-semibold pl-4 pr-6 py-2">
                    {
                        icon &&
                        <div className={`${text ? 'mr-2' : ''}`}>
                            {icon}
                        </div>
                    }
                    {
                        text &&
                        <>
                            {text}
                        </>
                    }
                </button>
            </div >
        </Link>
    )
}

export default TopNavbarItem
