import React from 'react'
import { Link } from 'react-router-dom';

const TopNavbarItem = (props) => {
    const { text, icon, link } = props;

    return (
        <div className="flex shrink-0">
            <button className="flex items-center text-slate-700 font-semibold pl-4 pr-6 py-2 hover:rounded-full">
                {
                    text != false &&
                    <div className='mr-2'>
                        {icon}
                    </div>
                }
                {
                    text == false &&
                    <div>
                        {icon}
                    </div>
                }
                <Link to={link}>
                    {text}
                </Link>
            </button>
        </div >
    )
}

export default TopNavbarItem
