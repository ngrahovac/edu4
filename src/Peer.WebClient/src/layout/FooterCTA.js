import React from 'react'
import PageTitle from './PageTitle'
import CTA from '../comps/landing/CTA'

const FooterCTA = () => {
    return (
        <div className='flex flex-col gap-y-2'>
            <p className='text-2xl font-semibold'>Stay in the Loop</p>
            <p className='text-gray-300 mb-4'>Subscribe to our newsletter and receive updates on platform development <br></br> and early access to public beta</p>

            <form
                onSubmit={e => e.preventDefault()}
                className='flex flex-row rounded-full border-2 border-gray-400 justify-between px-2 py-1 w-full focus-within:border-2 focus-within:border-indigo-500'>
                <input
                    type="email"
                    required={true}
                    name="title"
                    placeholder='Your email here..'
                    className="bg-transparent md:text-lg border-transparent focus:border-transparent focus:ring-0 caret-indigo-500">
                </input>
                <button
                    className='md:text-lg font-semibold px-4 rounded-full text-gray-200 hover:text-indigo-200 cursor-pointer'>
                    Subscribe
                </button>
            </form>
        </div>
    )
}

export default FooterCTA