import React from 'react'
import { Link } from 'react-router-dom';

const TopNavbarItem = (props) => {
    const { text, icon, link } = props;

    return (
        <div className="flex shrink-0">
            <button className="flex items-center text-base text-slate-700 uppercase tracking-widest">
                {icon}
                <Link to={link}>
                    {text}
                </Link>
            </button>
        </div>
    )
}

export default TopNavbarItem
