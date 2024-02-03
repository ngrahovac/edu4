import React, { useState } from 'react'
import { Outlet } from 'react-router-dom'
import Avatar from './Avatar'
import LogoIcon from './LogoIcon'
import BorderlessButton from '../buttons/BorderlessButton'
import { Link } from 'react-router-dom'
import AccentButton from '../buttons/AccentButton'
import TopNavbarContextMenu from './TopNavbarContextMenu'
import { useAuth0 } from '@auth0/auth0-react'
import GlobalSearch from './GlobalSearch'

const TopNavbar = () => {
    const [topNavbarContextMenuHidden, setTopNavbarContextMenuHidden] = useState(true);

    const { user, isAuthenticated } = useAuth0();

    function toggleTopNavbarContextMenuVisibility() {
        setTopNavbarContextMenuHidden(!topNavbarContextMenuHidden);
    }

    const topNavbarContextMenu = (
    <div className='w-64 fixed top-14 right-8 z-50'>
        <TopNavbarContextMenu
            hidden={topNavbarContextMenuHidden}>
        </TopNavbarContextMenu>
    </div>);

    return (isAuthenticated &&
        <>
            {topNavbarContextMenu}

            <div className='fixed w-full h-16 bg-gray-100 flex justify-between px-8 z-40'>
                {/* navbar items on the left */}
                <div className='flex items-center gap-x-4'>
                    <LogoIcon></LogoIcon>

                    <GlobalSearch></GlobalSearch>
                </div>

                {/* navbar items on the right */}
                <div className='flex items-center justify-end gap-x-4'>
                    {/* publish */}
                    <Link to="publish">
                        <AccentButton
                            text="Publish"
                            icon={<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                <path strokeLinecap="round" strokeLinejoin="round" d="M10.34 15.84c-.688-.06-1.386-.09-2.09-.09H7.5a4.5 4.5 0 110-9h.75c.704 0 1.402-.03 2.09-.09m0 9.18c.253.962.584 1.892.985 2.783.247.55.06 1.21-.463 1.511l-.657.38c-.551.318-1.26.117-1.527-.461a20.845 20.845 0 01-1.44-4.282m3.102.069a18.03 18.03 0 01-.59-4.59c0-1.586.205-3.124.59-4.59m0 9.18a23.848 23.848 0 018.835 2.535M10.34 6.66a23.847 23.847 0 008.835-2.535m0 0A23.74 23.74 0 0018.795 3m.38 1.125a23.91 23.91 0 011.014 5.395m-1.014 8.855c-.118.38-.245.754-.38 1.125m.38-1.125a23.91 23.91 0 001.014-5.395m0-3.46c.495.413.811 1.035.811 1.73 0 .695-.316 1.317-.811 1.73m0-3.46a24.347 24.347 0 010 3.46" />
                            </svg>}>
                        </AccentButton>
                    </Link>

                    {/* discover */}
                    <Link to="discover">
                        <BorderlessButton
                            text="Discover"
                            icon={<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                            <path strokeLinecap="round" strokeLinejoin="round" d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z" />
                          </svg>
                          }>
                        </BorderlessButton>
                    </Link>

                    {/* applications */}
                    <Link to="applications">
                        <BorderlessButton
                            text="Applications"
                            icon={<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                <path strokeLinecap="round" strokeLinejoin="round" d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75" />
                            </svg>}>
                        </BorderlessButton>
                    </Link>

                    {/* notifications */}
                    <BorderlessButton
                        text=""
                        icon={<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth="1.5" stroke="currentColor" className="w-6 h-6">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M14.857 17.082a23.848 23.848 0 005.454-1.31A8.967 8.967 0 0118 9.75v-.7V9A6 6 0 006 9v.75a8.967 8.967 0 01-2.312 6.022c1.733.64 3.56 1.085 5.455 1.31m5.714 0a24.255 24.255 0 01-5.714 0m5.714 0a3 3 0 11-5.714 0" />
                        </svg>}>
                    </BorderlessButton>

                    <div onClick={toggleTopNavbarContextMenuVisibility}>
                        {
                            <img src={user.picture} width={36} height={36} className='rounded-full antialiased' referrerPolicy='no-referrer'></img>
                        }
                    </div>
                </div>
            </div>

            <Outlet></Outlet>
        </>
    )
}

export default TopNavbar