import React from 'react'
import { Outlet } from 'react-router-dom'
import LogoutButton from '../account/LogoutButton'
import Search from './Search'
import TopNavbarItem from './TopNavbarItem'

const TopNavbar = () => {
    return (
        <>
            <div
                className='fixed w-full h-16 bg-stone-50 flex flex-row justify-between px-8 z-40'>
                {/* 1st item group */}
                <div
                    className='flex flex-row basis-1/3 px-4 items-center'>
                    <TopNavbarItem
                        text=""
                        icon={
                            <div className="mr-2">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth="1.5" stroke="currentColor" class="w-6 h-6">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 12l8.954-8.955c.44-.439 1.152-.439 1.591 0L21.75 12M4.5 9.75v10.125c0 .621.504 1.125 1.125 1.125H9.75v-4.875c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21h4.125c.621 0 1.125-.504 1.125-1.125V9.75M8.25 21h8.25" />
                                </svg>
                            </div>
                        }
                        link="#">
                    </TopNavbarItem>

                    <div className='ml-4'>
                        <Search></Search>
                    </div>
                </div>

                {/* 2nd item group */}
                <div
                    className='flex flex-row basis-1/3 justify-end px-4 items-center'>
                    {/* notifications */}
                    <div className="mr-2">
                        <TopNavbarItem
                            text=""
                            icon={
                                <div className="mr-2">
                                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth="1.5" stroke="currentColor" className="w-6 h-6">
                                        <path strokeLinecap="round" strokeLinejoin="round" d="M14.857 17.082a23.848 23.848 0 005.454-1.31A8.967 8.967 0 0118 9.75v-.7V9A6 6 0 006 9v.75a8.967 8.967 0 01-2.312 6.022c1.733.64 3.56 1.085 5.455 1.31m5.714 0a24.255 24.255 0 01-5.714 0m5.714 0a3 3 0 11-5.714 0" />
                                    </svg>
                                </div>
                            }
                            link="#">
                        </TopNavbarItem>
                    </div>

                    {/* account */}
                    <TopNavbarItem
                        text=""
                        icon={
                            <div className="mr-2">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth="1.5" stroke="currentColor" className="w-6 h-6">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.963 0a9 9 0 10-11.963 0m11.963 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z" />
                                </svg>
                            </div>
                        }
                        link="#">
                    </TopNavbarItem>

                    <LogoutButton></LogoutButton>
                </div>
            </div>
            <Outlet></Outlet>
        </>
    )
}

export default TopNavbar