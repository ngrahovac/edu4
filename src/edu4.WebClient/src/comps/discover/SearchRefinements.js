import React, { useState, useEffect } from 'react'
import SubsectionTitle from '../../layout/SubsectionTitle'
import Hat from '../hats/Hat'
import SelectedHat from '../hats2/SelectedHat';
import _ from 'lodash';
import ClearSearchFilter from './ClearSearchFilter';

const SearchRefinements = (props) => {

  const {
    onModalClosed,
    onSearchKeywordChanged,
    onSelectedSortChanged,
    onSelectedHatChanged,
  } = props;

  const hats = [
    {
      "type": "Student",
      "parameters": {
        "studyField": "Software Engineering",
        "academicDegree": "Bachelor's"
      }
    },
    {
      "type": "Academic",
      "parameters": {
        "researchField": "Electronics Engineering"
      }
    },
  ];

  const [searchKeyword, setSearchKeyword] = useState(undefined);

  const [selectedSort, setSelectedSort] = useState(undefined);

  const [selectedHat, setSelectedHat] = useState(undefined);

  function clearSearchKeyword() {
    setSearchKeyword(undefined);
  }

  function clearSelectedSort() {
    setSelectedSort(undefined);
  }

  function clearSelectedHat() {
    setSelectedHat(undefined);
  }

  function onFormChange(e) {
    if (e.target.name == "keyword") {
      setSearchKeyword(e.target.value);
    } else if (e.target.name == "sort") {
      setSelectedSort(e.target.value);
    }
  }

  function onClosed() {
    onModalClosed();
  }

  useEffect(() => {
    onSearchKeywordChanged(searchKeyword);
  }, [searchKeyword])

  useEffect(() => {
    onSelectedSortChanged(selectedSort);
  }, [selectedSort])

  useEffect(() => {
    onSelectedHatChanged(selectedHat);
  }, [selectedHat])

  return (
    <div className='h-screen bg-gray-100 w-full px-16 py-64 relative'>
      <form onChange={onFormChange} onSubmit={e => e.preventDefault()}>
        {/* Search */}
        <label>
          <div className='flex flex-row shrink-0 items-center content-center mb-2'>
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" className="w-6 h-6 mr-1">
              <path stroke-linecap="round" stroke-linejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
            </svg>
            <p className='text-md font-semibold'>Search</p>
          </div>
          <input
            type="text"
            name="keyword"
            value={searchKeyword == undefined ? "" : searchKeyword}
            className="w-full mt-1 block rounded-full border-gray-300 focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></input>
          <ClearSearchFilter onClick={clearSearchKeyword}></ClearSearchFilter>
        </label>

        {/* Sort */}

        <div className='flex flex-col mt-20'>
          <div className='flex flex-row shrink-0 items-center content-center mb-2'>
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" className="w-6 h-6 mr-1">
              <path stroke-linecap="round" stroke-linejoin="round" d="M3 7.5L7.5 3m0 0L12 7.5M7.5 3v13.5m13.5 0L16.5 21m0 0L12 16.5m4.5 4.5V7.5" />
            </svg>
            <p className='text-md font-semibold'>Sort</p>
          </div>

          <div className='flex flex-col'>
            <label className='block'>
              <input
                type="radio"
                name="sort"
                value="asc"
                checked={selectedSort == "asc"}
                className="mr-2">
              </input>
              Oldest first
            </label>

            <label className='block'>
              <input
                type="radio"
                name="sort"
                value="desc"
                checked={selectedSort == "desc"}
                className="mr-2">
              </input>
              Newest first
            </label>
          </div>

          <ClearSearchFilter onClick={clearSelectedSort}></ClearSearchFilter>
        </div>

        {/* Filter */}
        <div className='mt-20'>
          <label>
            <div className='flex flex-row shrink-0 items-center content-center mb-2'>
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" className="w-6 h-6 mr-1">
                <path stroke-linecap="round" stroke-linejoin="round" d="M12 3c2.755 0 5.455.232 8.083.678.533.09.917.556.917 1.096v1.044a2.25 2.25 0 01-.659 1.591l-5.432 5.432a2.25 2.25 0 00-.659 1.591v2.927a2.25 2.25 0 01-1.244 2.013L9.75 21v-6.568a2.25 2.25 0 00-.659-1.591L3.659 7.409A2.25 2.25 0 013 5.818V4.774c0-.54.384-1.006.917-1.096A48.32 48.32 0 0112 3z" />
              </svg>
              <p className='text-md font-semibold'>Filter</p>
            </div>
            <p>Projects looking for:</p>
          </label>

          {/* hats go here */}
          <div>
            {
              hats.map(h => <div key={Math.floor((Math.random() * 10000))}>
                {
                  _.isEqual(h, selectedHat) ?

                    <div onClick={() => setSelectedHat(h)}>
                      <SelectedHat hat={h}></SelectedHat>
                    </div> :

                    <div onClick={() => setSelectedHat(h)}>
                      <Hat hat={h}></Hat>
                    </div>
                }
              </div>)
            }
          </div>

          <ClearSearchFilter onClick={clearSelectedHat}></ClearSearchFilter>
        </div>
      </form>

      <button
        onClick={onModalClosed}
        className='absolute right-4 top-20'>
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
          <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>
  )
}

export default SearchRefinements